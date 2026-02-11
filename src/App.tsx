import { BrowserRouter } from "react-router";
import { AppRoutes } from "./app/routes";
import { ToastContainer } from "react-toastify";
import ErrorBoundaryWithNotifier from "./shared/errors/ErrorBoundaryWithNotifier";
import { AutenticacionProvider } from "./app/auth/Estado/AutenticacionProvider";
import TokenWatcher from "./app/auth/Monitoreo/TokenWatcher";
function App() {
  return (
    <>
      <ErrorBoundaryWithNotifier
        fallback={
          <div style={{ padding: 32 }}>
            <h2>Ocurrió un error inesperado</h2>
            <p>Intenta recargar la aplicación.</p>
            <button onClick={() => window.location.reload()}>
              Recargar
            </button>
          </div>
        }
      >
        <BrowserRouter>
        <AutenticacionProvider>
               <TokenWatcher/> 
             <AppRoutes />
        </AutenticacionProvider>
        </BrowserRouter>
      </ErrorBoundaryWithNotifier>

      {/* Infraestructura de notificaciones (fuera del boundary) */}
      <ToastContainer position="top-right" autoClose={5000} />
    </>
  );
}

export default App;

