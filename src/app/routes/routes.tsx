import type { RouteObject } from "react-router";
import LoginPage from "../../modules/login/pages/LoginPage";
import OTPVerifyPage from "../../modules/OTP/pages/OTPVerifyPage";
import RecuperarPasswordPage from "../../modules/RecoveryPassword/pages/RecuperarPasswordPage";
import OTPVerifyRecoveryPaswPage from "../../modules/OTP/pages/OTPVerifyRecoveryPaswPage";
import CambiarPasswordPage from "../../modules/RecoveryPassword/pages/CambiarPasswordPage";

import RutaProtegida from "../auth/ProteccionRuta/RutaProtegida";

import DashboardLayout from "../../modules/dashboard/components/DashboardLayout";
import DashboardHome from "../../modules/dashboard/pages/DashboardHome";
import WorkflowPage from "../../modules/Workflow/pages/WorkflowPage";
import RadicacionRoutePage from "../../modules/radicacion/pages/RadicacionRoutePage";

export const loginRoutes: RouteObject[] = [
  { path: "/", element: <LoginPage /> },
  { path: "/LoginPage", element: <LoginPage /> },
  { path: "/verificar-otp", element: <OTPVerifyPage /> },
  { path: "/recovery-password/forgot-password", element: <RecuperarPasswordPage /> },
  { path: "/RecoveryPassword/forgot-password/verify", element: <OTPVerifyRecoveryPaswPage /> },
  { path: "/RecoveryPassword/cambiar-password", element: <CambiarPasswordPage /> },

  {
    element: <RutaProtegida claims={[]} />,
    children: [
      {
        path: "/dashboard",
        element: <DashboardLayout />,
        handle: { restricted: true },
        children: [
          { index: true, element: <DashboardHome /> },
          {
            path: "workflow",
            element: <WorkflowPage />,
          },
          {
            path: "radicacion",
            element: <RadicacionRoutePage />,
          },
        ],
      },
    ],
  },
];
