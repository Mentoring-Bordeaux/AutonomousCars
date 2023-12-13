<script>
const config = useRuntimeConfig();
let timer = null;

export default {
  data() {
    return {
      departure: '',
      searchResults: [],
    };
  },
  methods: {
    handleInput() {
      this.debounceRequest();
    },
    async debounceRequest() {
      if (this.departure === '') {
        this.searchResults = [];
        return; // Vérification si le champ est vide
      }

      clearTimeout(timer);
      timer = setTimeout(this.sendRequest, 500); // Attendre 500ms avant d'envoyer la requête
    },
    async sendRequest() {
      try {
        const response = await fetch(`https://atlas.microsoft.com/search/fuzzy/json?&api-version=1.0&subscription-key=${config.public.azureMapsSubscriptionKey}&language=en-US&countrySet=FR&query=${this.departure}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json', // Si nécessaire
          },
        });

        if (response.ok) {
          const responseData = await response.json();
          console.log('Réponse du serveur :', responseData);
          this.searchResults = responseData.results.map(result => ({
            id: result.id,
            name: `${result.address.streetNumber || ''} ${result.address.streetName || ''}`,
            postalCode: `${result.address.postalCode || ''}`,
            municipality: `${result.address.municipality || ''}`
          }));
          // Traitez la réponse du serveur si nécessaire
        } else {
          console.error('Erreur lors de la requête :', response.statusText);
          // Gérez les erreurs de la requête
        }
      } catch (error) {
        console.error('Erreur :', error);
        // Gérez les erreurs d'exceptions
      }
    },
    selectResult(result) {
      // Gérer la sélection d'un résultat, par exemple remplir le champ avec le résultat sélectionné
      this.departure = result.name + ', ' + result.postalCode + ', ' +  result.municipality;
      this.searchResults = []; // Vider la liste des résultats après la sélection
    }
  }

};
</script>

<template>
  <form @submit.prevent="submitForm" class="max-w-lg mx-auto mt-8 p-6 bg-white shadow-md rounded">
    <div class="mb-4">
      <label for="departure" class="block text-gray-700 font-semibold mb-2">Arriver à :</label>
      <input type="text" id="departure" v-model="departure" @input="sendRequest" class="w-full border rounded-md py-2 px-3 text-white-700 leading-tight focus:outline-none focus:shadow-outline" required>
    </div>

    <ul v-if="searchResults.length" class="border rounded-md mt-2">
      <li v-for="result in searchResults" :key="result.id" class="py-2 px-3 cursor-pointer hover:bg-black-200 text-black" @click="selectResult(result)">
        <div>
          <span>{{ result.name }}</span>
          <br>
          <span>{{ result.postalCode }} {{ result.municipality }}</span>
        </div>
      </li>
    </ul>

    <button type="submit" class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline">Soumettre</button>
  </form>
</template>