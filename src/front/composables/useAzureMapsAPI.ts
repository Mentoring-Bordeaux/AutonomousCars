import { TokenCredential } from "@azure/core-auth";
import MapsSearch, { isUnexpected } from "@azure-rest/maps-search";

export function useAzureMapsAPI() {
  const config = useRuntimeConfig();
  const { getMapToken } = useAzureMaps();
  const credential: TokenCredential = {
    getToken: async (scopes) => {
      const { expiresOn, token } = await getMapToken();

      return {
        token,
        expiresOnTimestamp: expiresOn,
      };
    },
  };

  const client = MapsSearch(credential, config.public.azureMapsClientId);

  async function fetchAdresses(query: string) {
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
    }
  }
  return { fetchAdresses };
}
