import api from "./api";

export type WebsiteProfile = {
  name: string;
  code: string;
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
};

export type BannerItem = {
  id: number;
  caption?: string;
  sortOrder: number;
  imageUrl: string;
};

export type PracticeAreaItem = {
  id: number;
  title: string;
  description?: string;
  sortOrder: number;
};

export type AdvocateItem = {
  id: number;
  fullName: string;
  designation?: string;
  bio?: string;
  sortOrder: number;
  photoUrl?: string;
};

export const websiteService = {
  getProfile: () => api.get<WebsiteProfile>("/website/profile"),
  updateProfile: (payload: Omit<WebsiteProfile, "name" | "code">) =>
    api.put<WebsiteProfile>("/website/profile", payload),
  listBanners: () => api.get<BannerItem[]>("/website/banners"),
  uploadBanner: (file: File, caption: string, sortOrder: number) => {
    const form = new FormData();
    form.append("file", file);
    form.append("caption", caption);
    form.append("sortOrder", String(sortOrder));
    return api.post<BannerItem>("/website/banners", form);
  },
  updateBanner: (id: number, caption: string, sortOrder: number) =>
    api.put(`/website/banners/${id}`, { caption, sortOrder }),
  deleteBanner: (id: number) => api.delete(`/website/banners/${id}`),
  listPracticeAreas: () => api.get<PracticeAreaItem[]>("/website/practice-areas"),
  createPracticeArea: (payload: { title: string; description?: string; sortOrder: number }) =>
    api.post<PracticeAreaItem>("/website/practice-areas", payload),
  updatePracticeArea: (id: number, payload: { title: string; description?: string; sortOrder: number }) =>
    api.put(`/website/practice-areas/${id}`, payload),
  deletePracticeArea: (id: number) => api.delete(`/website/practice-areas/${id}`),
  listAdvocates: () => api.get<AdvocateItem[]>("/website/advocates"),
  createAdvocate: (payload: { fullName: string; designation?: string; bio?: string; sortOrder: number }) =>
    api.post<AdvocateItem>("/website/advocates", payload),
  updateAdvocate: (id: number, payload: { fullName: string; designation?: string; bio?: string; sortOrder: number }) =>
    api.put(`/website/advocates/${id}`, payload),
  uploadAdvocatePhoto: (id: number, file: File) => {
    const form = new FormData();
    form.append("file", file);
    return api.post<{ photoUrl: string }>(`/website/advocates/${id}/photo`, form);
  },
  deleteAdvocate: (id: number) => api.delete(`/website/advocates/${id}`),
};
