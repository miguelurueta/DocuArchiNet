import type { RouteObject } from "react-router";
import LoginPage from "../../modules/login/pages/LoginPage";
import OTPVerifyPage from "../../modules/OTP/pages/OTPVerifyPage";
import RecuperarPasswordPage from "../../modules/RecoveryPassword/pages/RecuperarPasswordPage";
import OTPVerifyRecoveryPaswPage from "../../modules/OTP/pages/OTPVerifyRecoveryPaswPage";
import CambiarPasswordPage from "../../modules/RecoveryPassword/pages/CambiarPasswordPage";
import DashboardPage from "../../modules/dashboard/pages/DashboardPage";
import ModuleDetailPage from "../../modules/dashboard/pages/ModuleDetailPage";
export const loginRoutes: RouteObject[] = [
    { path: "/", element: <LoginPage /> },
    { path: "/LoginPage", element: <LoginPage /> },
    { path: "/verificar-otp",element: <OTPVerifyPage />},
    {path: "/recovery-password/forgot-password",element: <RecuperarPasswordPage />},
    {path:"/RecoveryPassword/forgot-password/verify", element:<OTPVerifyRecoveryPaswPage/>},
    {path:"/RecoveryPassword/cambiar-password", element:<CambiarPasswordPage/>},
    {path: "/dashboard", element: <DashboardPage />},
    {path: "/dashboard/module/:nodeId", element: <ModuleDetailPage />}
];
