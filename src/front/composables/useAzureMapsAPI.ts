import { AzureKeyCredential, TokenCredential } from "@azure/core-auth";
import MapsSearch, { isUnexpected } from "@azure-rest/maps-search";
import { ResourceCredential } from "~/models/resourceCredential";
import { FetchAddressesFunction, Address } from "~/models/address";

export async function useAzureMapsAPI(): Promise<{ fetchAdresses: FetchAddressesFunction }> {
  const config = useRuntimeConfig();
  const { getMapCredential } = useAzureMaps();

  const { clientId, accessToken: { token, expiresOn } } = await getMapCredential();

  const tokenCredential: TokenCredential = {
    getToken: async (scopes) => {
      return {
        token: token,
        expiresOnTimestamp: expiresOn,
      };
    },
  }; 

  const client = MapsSearch(tokenCredential, clientId);

  async function fetchAdresses(query: string): Promise<Address[]> {
    try {
      const response = await client.path("/search/fuzzy/{format}", "json").get({
        queryParameters: {
          query,
          countrySet: ["FR"],
        },
      });

      if (isUnexpected(response)) {
        throw response.body.error;
      }

      return response.body.results.map((result) => ({
        id: result.id,
        name: `${result.address.streetNumber || ""} ${
          result.address.streetName || ""
        }`,
        postalCode: `${result.address.postalCode || ""}`,
        municipality: `${result.address.municipality || ""}`,
        position: result.position,
      }));
    } catch (error) {
      console.error(error);
      return [];
    }
  }
  return { fetchAdresses };
}
