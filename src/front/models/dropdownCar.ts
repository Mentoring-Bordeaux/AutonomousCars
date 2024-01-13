import { Vehicle } from "~/models/Vehicle"

export type carItem = {
    name: string, 
    icon: string
    vehicle: Vehicle | null;
}