import type { AccessToken } from "~/models/accessToken";

export function useAzureMaps() {
  async function getToken() {
    const {
      data: { value },
    } = await useFetch<AccessToken>("api/token/map");

    if (!value) {
      throw new Error("Error while retrieving azure maps token");
    }

    return value!;
  }

  return { getToken };
}
