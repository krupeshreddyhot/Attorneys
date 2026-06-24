import axios from "axios";

export type FirmLanding = {
  code: string;
  name: string;
  addressLine?: string;
  city?: string;
  phone?: string;
  email?: string;
  heroTagline?: string;
  heroSubtitle?: string;
  aboutTitle?: string;
  aboutBody?: string;
  aboutHighlightTitle?: string;
  aboutHighlightBody?: string;
  banners: { id: number; caption?: string; sortOrder: number; imageUrl: string }[];
  practiceAreas: { id: number; title: string; description?: string; sortOrder: number }[];
  advocates: {
    id: number;
    fullName: string;
    designation?: string;
    bio?: string;
    sortOrder: number;
    photoUrl?: string;
  }[];
};

const publicApi = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5063/api",
});

export async function fetchFirmLanding(firmCode: string): Promise<FirmLanding> {
  const { data } = await publicApi.get<FirmLanding>(`/public/firms/${firmCode.toUpperCase()}/landing`);
  return data;
}
