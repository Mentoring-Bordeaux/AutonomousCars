import type { Vehicle } from "~/models/Vehicle";
import type { Position } from "~/models/Vehicle";

export const useVehiclesListStore = defineStore('vehiclesListStore', {
    state: () => ({
      vehiclesList: [] as Vehicle[],
      isLoaded: false
    }),
    actions: {
       async initVehiclesDevices() {
      
        const initialPosition: Position = {
          type: "Point",
          coordinates: [10, 0] 
        };
        
        try {
          const response = await fetch('/api/ListingDevices/getAllDevices'); 
          const devices = await response.json();

          if (devices.status === 'Success') {
            this.vehiclesList = devices.deviceNames.map((deviceName: string) => ({
              carId: deviceName,
              position: initialPosition,
              available: false
            }));            
            this.isLoaded = true;
          } else {
            console.error('Error of status in the API answer');
          }
        } catch (error) {
          console.error('Error during api request', error);
        }
      }
    }

  })