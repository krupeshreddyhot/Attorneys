import { createContext, useContext, useMemo, useState } from "react";



type UserClaims = {

  userName: string;

  role: string;

  tenantId: number;

  firmCode: string | null;

  firmName: string | null;

};



interface AuthContextType {

  token: string | null;

  user: UserClaims | null;

  login: (token: string) => void;

  logout: () => void;

  isAuthenticated: boolean;

  isAdministrator: boolean;

  isSuperAdmin: boolean;

}



const AuthContext = createContext<AuthContextType | null>(null);



const parseJwt = (token: string): Record<string, unknown> | null => {

  try {

    const payload = token.split(".")[1];

    if (!payload) return null;

    const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");

    const decoded = atob(normalized.padEnd(Math.ceil(normalized.length / 4) * 4, "="));

    return JSON.parse(decoded);

  } catch {

    return null;

  }

};



const getUserClaims = (token: string | null): UserClaims | null => {

  if (!token) return null;

  const claims = parseJwt(token);

  if (!claims) return null;

  return {

    userName: String(claims.userName ?? claims.unique_name ?? claims.name ?? claims.sub ?? ""),

    role: String(claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? claims.role ?? ""),

    tenantId: Number(claims.TenantId ?? 0),

    firmCode: claims.FirmCode ? String(claims.FirmCode) : null,

    firmName: claims.FirmName ? String(claims.FirmName) : null,

  };

};



export const AuthProvider = ({ children }: { children: React.ReactNode }) => {

  const [token, setToken] = useState<string | null>(localStorage.getItem("token"));



  const handleLogin = (nextToken: string) => {

    localStorage.setItem("token", nextToken);

    setToken(nextToken);

  };



  const handleLogout = () => {

    localStorage.removeItem("token");

    setToken(null);

  };



  const user = useMemo(() => getUserClaims(token), [token]);

  const isAdministrator = user?.role === "Administrator";

  const isSuperAdmin = user?.role === "SuperAdmin";



  const value = useMemo(

    () => ({

      token,

      user,

      login: handleLogin,

      logout: handleLogout,

      isAuthenticated: Boolean(token),

      isAdministrator,

      isSuperAdmin,

    }),

    [token, user, isAdministrator, isSuperAdmin],

  );



  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;

};



export const useAuth = () => {

  const ctx = useContext(AuthContext);

  if (!ctx) throw new Error("useAuth must be used within AuthProvider");

  return ctx;

};

