export function publicMediaUrl(relativePath: string): string {
  const apiBase = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5063/api";
  const serverRoot = apiBase.replace(/\/api\/?$/, "");
  return `${serverRoot}${relativePath}`;
}
