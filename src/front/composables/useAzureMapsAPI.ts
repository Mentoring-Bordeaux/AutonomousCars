import type { TokenCredential } from "@azure/core-auth";
import MapsSearch, { isUnexpected } from "@azure-rest/maps-search";
import type { FetchAddressesFunction, Address } from "~/models/address";

export async function useAzureMapsAPI(): Promise<{ fetchAdresses: FetchAddressesFunction }> {
  const { getMapCredential } = useAzureMaps();

  const { clientId } = await getMapCredential();
  
    const tokenCredential: TokenCredential = {
        getToken: async () => {
        const { accessToken : {token, expiresOn} } = await getMapCredential();
        return {
          token,
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
