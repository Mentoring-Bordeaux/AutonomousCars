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
      await useFetch('/api/ListingDevices/', {
        onRequestError: () => {
          this.isError = true;
          this.isLoaded = true;
        },
        onResponse: ({ response }) => {
          if (response.status == 200) {
            const deviceNames = response._data.deviceNames;

            this.vehiclesList = deviceNames.map((deviceName: string) => ({
              carId: deviceName,
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
