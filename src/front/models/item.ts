import { LocationQuery, LocationQueryRaw } from "vue-router"

export type section = {
    icon: string,
    title: string,
    queryParams: LocationQueryRaw,
    sideBar: boolean
}