import { Vehicle } from "~/models/Vehicle";
import { Position } from "~/models/Vehicle";

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
            for (const deviceName of devices.deviceNames) {
              const vehicleLocation: Vehicle = {
                carId: deviceName, 
                position: initialPosition,
                available: false, 
              };
              this.vehiclesList.push(vehicleLocation);  
            }
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