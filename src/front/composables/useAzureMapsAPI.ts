export function useAzureMapsAPI() {
    async function fetchAdresses(query: String) {
        const config = useRuntimeConfig();
        try {
            const response = await fetch(`https://atlas.microsoft.com/search/fuzzy/json?&api-version=1.0&subscription-key=${config.public.azureMapsSubscriptionKey}&language=en-US&countrySet=FR&query=${query}`, {
                method: 'GET',
                headers: {
                  'Content-Type': 'application/json', // Si nécessaire
                },
              });
              if (response.ok) {
                const responseData = await response.json();
                console.log('Réponse du serveur :', responseData);
                return responseData
            }else {
                console.error('Erreur lors de la requête :', response.statusText);
                // Gérez les erreurs de la requête
            }   
        }catch (error) {
            console.error('Erreur :', error);
            // Gérez les erreurs d'exceptions
        } 
    }
    return { fetchAdresses }
}