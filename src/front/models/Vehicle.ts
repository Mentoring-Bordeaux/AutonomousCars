export type Position = {
    type: string,
    coordinates: number[]
}

export type Vehicle = {
    carId: string,
    position: Position,
    available: boolean
}