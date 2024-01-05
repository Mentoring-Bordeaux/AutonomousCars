import type { ResourceCredential } from "~/models/resourceCredential";

export function useAzureMaps() {
  async function getMapCredential() {
    const { data } = await useFetch<ResourceCredential>("api/credentials/map");

    if (!data.value) {
      throw new Error("Error while retrieving azure maps credentials");
    }

    return data.value;
  }

  return { getMapCredential };
}
