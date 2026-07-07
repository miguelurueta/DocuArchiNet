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
import { RADICACION_ROUTE_SEGMENTS } from "../../modules/radicacion/routes/radicacionRoutes";
import GestionCorrespondenciaLayout from "../../modules/gestionCorrespondencia/layout/GestionCorrespondenciaLayout";
import GestionCorrespondenciaRoute from "../../modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute";
import GestionRespuesta from "../../modules/gestionCorrespondencia/pages/GestionRespuesta";
import PlaywrightEmbedPdfPage from "../pages/PlaywrightEmbedPdfPage";
import AppDigitalizadorSandboxPage from "../pages/AppDigitalizadorSandboxPage";

export const loginRoutes: RouteObject[] = [
  { path: "/", element: <LoginPage /> },
  { path: "/LoginPage", element: <LoginPage /> },
  { path: "/__playwright/embedpdf", element: <PlaywrightEmbedPdfPage /> },
  { path: "/__sandbox/app-digitalizador", element: <AppDigitalizadorSandboxPage /> },
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
            path: RADICACION_ROUTE_SEGMENTS.root,
            element: <RadicacionRoutePage />,
          },
          {
            path: `${RADICACION_ROUTE_SEGMENTS.root}/${RADICACION_ROUTE_SEGMENTS.registro}/:idEstadoRadicado/${RADICACION_ROUTE_SEGMENTS.documentos}`,
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
