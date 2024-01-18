import * as atlas from "azure-maps-control";

export type Vehicle = {
    carId: string,
    carName: string,
    marker: atlas.HtmlMarker,
    available: boolean
}