import type { Vehicle } from "~/models/Vehicle";
import type { Position } from "~/models/Vehicle";

export const useVehiclesListStore = defineStore('vehiclesListStore', {
  state: () => ({
    vehiclesList: [] as Vehicle[],
    isLoaded: false,
    isRequestError: false,
    isResponseError: false
  }),
  actions: {
    async initVehiclesDevices() {
      const initialPosition: Position = {
        type: "Point",
        coordinates: [10, 0] 
      };

      await useFetch('/api/ListingDevices/getAllDevices', {
        onRequestError : () => {
          this.isRequestError = true;
        },
        onResponse: ({ response }) => {
          const deviceNames = response._data.deviceNames;

          this.vehiclesList = deviceNames.map((deviceName: string) => ({
            carId: deviceName,
            position: initialPosition,
            available: false
          }));            

          this.isLoaded = true;
        },
        onResponseError: () => {
          this.isResponseError = true;
        }
      });
    }
  }
});
