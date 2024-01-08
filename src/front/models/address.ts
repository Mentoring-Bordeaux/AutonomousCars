export type Address = {
    id: string;
    name: string;
    postalCode: string;
    municipality: string;
    position: {lat: number, lon: number}
  };

export type FetchAddressesFunction = (query: string) => Promise<Address[]>;