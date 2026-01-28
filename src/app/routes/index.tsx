import { useRoutes } from "react-router";
import { loginRoutes } from "./routes";
export function AppRoutes() {
  return useRoutes(loginRoutes);
}
