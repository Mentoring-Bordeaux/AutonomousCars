export type Position = {lat: number, lon: number}

export type Address = {
    id: string;
    name: string;
    postalCode: string;
    municipality: string;
    position: Position
  };

export type FetchAddressesFunction = (query: string) => Promise<Address[]>;