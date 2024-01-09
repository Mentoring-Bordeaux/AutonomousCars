type Position = {
    type: String,
    coordinates: number[]
}

export type Vehicle = {
    carId: String,
    position: Position,
    available: boolean
}