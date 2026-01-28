import { BrowserRouter } from "react-router";
import { AppRoutes } from "./app/routes";
import { ToastContainer } from "react-toastify";
import ErrorBoundaryWithNotifier from "./shared/errors/ErrorBoundaryWithNotifier";

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
          <AppRoutes />
        </BrowserRouter>
      </ErrorBoundaryWithNotifier>

      {/* Infraestructura de notificaciones (fuera del boundary) */}
      <ToastContainer position="top-right" autoClose={5000} />
    </>
  );
}

export default App;

