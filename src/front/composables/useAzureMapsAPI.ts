import { AzureKeyCredential } from "@azure/core-auth"
import MapsSearch, { isUnexpected } from "@azure-rest/maps-search"


export function useAzureMapsAPI() {

    const config = useRuntimeConfig();
    const credential = new AzureKeyCredential(config.public.azureMapsSubscriptionKey)
    //const credential = new InteractiveBrowserCredential({clientId: config.public.azureMapsClientId}); 
    const client = MapsSearch(credential); 

    async function fetchAdresses(query: string) {
        try {
            const response = await client.path("/search/fuzzy/{format}", "json").get({
                queryParameters: {
                    query,
                    countrySet: ["FR"]
                }   
              });

            if (isUnexpected(response)) {
                throw response.body.error;
            }

            return response.body.results.map(result => ({
                id: result.id,
                name: `${result.address.streetNumber || ''} ${result.address.streetName || ''}`,
                postalCode: `${result.address.postalCode || ''}`,
                municipality: `${result.address.municipality || ''}`,
                position: result.position
              }));
              
        }catch (error) {
            console.error('Erreur :', error);
            // Gérez les erreurs d'exceptions
        } 
    }
    return { fetchAdresses }
}