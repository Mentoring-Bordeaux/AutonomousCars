type routeGeometry = {
    type: "LineString";
    coordinates: Array<[number, number]>;
  }
  
type routeProperties = {
    distance: number; // en kilomètres
    time: number; // en secondes
    carId: string;
  }


export type routeFeature = {
    type: "Feature";
    geometry: routeGeometry;
    properties: routeProperties;
  }

export type Routes = {
    id: string;
    routeFeature: routeFeature;
}
  