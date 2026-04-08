import type { RouteObject } from "react-router";
import LoginPage from "../../modules/login/pages/LoginPage";
import OTPVerifyPage from "../../modules/OTP/pages/OTPVerifyPage";
import RecuperarPasswordPage from "../../modules/RecoveryPassword/pages/RecuperarPasswordPage";
import OTPVerifyRecoveryPaswPage from "../../modules/OTP/pages/OTPVerifyRecoveryPaswPage";
import CambiarPasswordPage from "../../modules/RecoveryPassword/pages/CambiarPasswordPage";

import RutaProtegida from "../auth/ProteccionRuta/RutaProtegida";

import DashboardLayout from "../../modules/dashboard/components/DashboardLayout";
import DashboardHome from "../../modules/dashboard/pages/DashboardHome";
import WorkflowLayout from "../../modules/Workflow/layout/WorkflowLayout";
import Workflow from "../../modules/Workflow/pages/Workflow";
import WorkflowAsignacion from "../../modules/Workflow/pages/WorkflowAsignacion";
import WorkflowEnlace from "../../modules/Workflow/pages/WorkflowEnlace";
import WorkflowRoute from "../../modules/Workflow/routes/WorkflowRoute";
import RadicacionRoutePage from "../../modules/radicacion/pages/RadicacionRoutePage";
import GestionCorrespondenciaLayout from "../../modules/gestionCorrespondencia/layout/GestionCorrespondenciaLayout";
import GestionCorrespondenciaRoute from "../../modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute";
import GestionRespuesta from "../../modules/gestionCorrespondencia/pages/GestionRespuesta";

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
            element: <WorkflowLayout />,
            children: [
              {
                index: true,
                element: <Workflow />,
              },
              {
                path: "asignacion",
                element: (
                  <WorkflowRoute
                    drawerTitle="Asignacion de workflow"
                    drawerContent={<WorkflowAsignacion />}
                  />
                ),
              },
              {
                path: "enlace",
                element: (
                  <WorkflowRoute
                    drawerTitle="Enlace de workflow"
                    drawerContent={<WorkflowEnlace />}
                  />
                ),
              },
            ],
          },
          {
            path: "radicacion",
            element: <RadicacionRoutePage />,
          },
          {
            path: "gestion-correspondencia",
            element: <GestionCorrespondenciaLayout />,
            children: [
              {
                index: true,
                element: <GestionCorrespondenciaRoute />,
              },
              {
                path: "respuesta/:id",
                element: (
                  <GestionCorrespondenciaRoute
                    detailContent={<GestionRespuesta />}
                  />
                ),
              },
            ],
          },
        ],
      },
    ],
  },
];
