import type { AccessToken } from "~/models/accessToken";

export function useAzureMaps() {
  async function getMapToken() {
    const { data } = await useFetch<AccessToken>("api/token/map");

    if (!data.value) {
      throw new Error("Error while retrieving azure maps token");
    }

    return data.value;
  }

  return { getMapToken };
}
