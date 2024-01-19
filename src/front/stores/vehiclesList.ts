import type { Vehicle } from "~/models/Vehicle";
import * as atlas from "azure-maps-control";

const carPath = "/img/car_icon.png";

export const useVehiclesListStore = defineStore('vehiclesListStore', {
  state: () => ({
    vehiclesList: [] as Vehicle[],
    isLoaded: false,
    isError: false
  }),
  actions: {
    async initVehiclesDevices() {
      await $fetch('/api/ListingDevices/getAllDevices', {
        onRequestError: () => {
          this.isError = true;
          this.isLoaded = true;
        },
        onResponse: ({ response }) => {
          if (response.status === 200) {
            const deviceNames = response._data.deviceNames;
            const carNameList: string[] = ['Model S - Tesla', 'Model 3 - Tesla', 'Model X - Tesla', 'Model Y - Tesla', 'Model S Plaid - Tesla'];

            // this.getStatusFromVehicle(deviceNames);

            this.vehiclesList = deviceNames.map((deviceName: string) : Vehicle => ({
              carId: deviceName,
              carName: `${carNameList[Math.floor(Math.random() * carNameList.length)]} [${deviceName}]`,
              marker: new atlas.HtmlMarker({
                text: deviceName,
                position: [44.812721, -0.590740],
                htmlContent: `
                  <div class="relative w-11 h-11 rounded-full overflow-hidden bg-gray-200 shadow-md flex items-center justify-center border border-white">
                    <img id="carImage" src="${carPath}" class="w-11 h-11 object-contain rounded-full" />
                    <div class="bg-green fixed bottom-0 right-2 w-3 h-3 rounded-full border-2 border-white"></div>
                  </div>
                `
              }),
              available: false
            }));

          } else {
            this.isError = true;
          }
          this.isLoaded = true;
        },
        onResponseError: () => {
          this.isError = true;
          this.isLoaded = true;
        }
      });
    },
    // async getStatusFromVehicle(identifiers: String[]) {
    //   await $fetch('/api/itinerary/status', {
    //     method: 'POST',
    //     headers: {
    //       'Content-Type': 'application/json',
    //     },
    //     body: JSON.stringify({
    //       CarIdentifiers: identifiers
    //     }),
    //   });
    // }
  }
});