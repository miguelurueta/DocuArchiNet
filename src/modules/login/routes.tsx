import type { RouteObject } from "react-router";
import Login from "./components/Login";
export const accountRoutes: RouteObject[] = [
  {
    path: "/",
    element: ( 
        <Login />
    )
  },
  {
    path: "/account",
    element: (
     
        <Login />
      
    )
  }
];


