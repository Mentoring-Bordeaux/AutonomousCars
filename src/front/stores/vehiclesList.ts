import type { Vehicle } from "~/models/Vehicle";
import type { Position } from "~/models/Vehicle";

export const useVehiclesListStore = defineStore('vehiclesListStore', {
  state: () => ({
    vehiclesList: [] as Vehicle[],
    isLoaded: false,
    isError: false
  }),
  actions: {
    async initVehiclesDevices() {
      const initialPosition: Position = {
        type: "Point",
        coordinates: [10, 0]
      };
      await useFetch('/api/ListingDevices/getAllDevices', {
        onRequestError: () => {
          this.isError = true;
          this.isLoaded = true;
        },
        onResponse: ({ response }) => {
          if (response.status === 200) {
            const deviceNames = response._data.deviceNames;
            const carNameList: string[] = ['Model S - Tesla', 'Model 3 - Tesla', 'Model X - Tesla', 'Model Y - Tesla', 'Model S Plaid - Tesla'];

            this.vehiclesList = deviceNames.map((deviceName: string) => ({
              carId: deviceName,
              carName: `${carNameList[Math.floor(Math.random() * carNameList.length)]} [${deviceName}]`,
              position: initialPosition,
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
    }
  }
});
