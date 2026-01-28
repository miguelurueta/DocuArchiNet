import { jwtDecode } from "jwt-decode";
type JwtPayload = {
  exp: number;
};
export function tokenExpirado(token: string): boolean {
  try {
    const { exp } = jwtDecode<JwtPayload>(token);
    return Date.now() >= exp * 1000;
  } catch {
    return true;
  }
}
