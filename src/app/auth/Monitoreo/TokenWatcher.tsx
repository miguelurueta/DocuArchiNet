import { useContext, useEffect, useMemo, useRef, useState } from "react";
import { matchRoutes, useLocation, useNavigate } from "react-router-dom";
import AutenticacionContext from "../Estado/AutenticacionContext";
import {
  existeTokenRegistrado,
  tokenExpirado,
  finalizarSesionYRedirigir,
} from "../Infraestructura/ManejadorJWT";
import { authConfig } from "../Configuracion/config";
import styles from "../../styles/AvisoToken.module.css";
import useRenovarToken from "../Hoks/useRenovarToken";
import { routeConfig } from "../../routes/routeConfig";

type EstadoTokenWatcher = "idle" | "no_registrado" | "expirado" | "renovando";

function logInfo(msg: string, data?: unknown) {
  // ✅ Log uniforme. Evitar tokens/claims en producción.
  // eslint-disable-next-line no-console
  console.info(`[TokenWatcher] ${msg}`, data ?? "");
}
function logWarn(msg: string, data?: unknown) {
  // eslint-disable-next-line no-console
  console.warn(`[TokenWatcher] ${msg}`, data ?? "");
}
function logError(msg: string, data?: unknown) {
  // eslint-disable-next-line no-console
  console.error(`[TokenWatcher] ${msg}`, data ?? "");
}

export default function TokenWatcher() {
  const { refrescarClaims } = useContext(AutenticacionContext);
  const { renovarToken } = useRenovarToken();
  const navigate = useNavigate();
  const location = useLocation();

  const [estado, setEstado] = useState<EstadoTokenWatcher>("idle");
  const timeoutRef = useRef<number | null>(null);

  // ✅ Determinar si la ruta actual está restringida (requiere auth)
  const rutaRestringida = useMemo(() => {
    const matches = matchRoutes(routeConfig as any, location);
    return (matches ?? []).some(
      (m) => (m.route as any)?.handle?.restricted === true
    );
  }, [location]);

  // 🔒 Bloquear scroll cuando el overlay está activo
  useEffect(() => {
    if (estado === "expirado" || estado === "renovando") {
      document.body.style.overflow = "hidden";
    }
    return () => {
      document.body.style.overflow = "";
    };
  }, [estado]);

  // 🧹 Cleanup timeout
  useEffect(() => {
    return () => {
      if (timeoutRef.current) window.clearTimeout(timeoutRef.current);
    };
  }, []);

  useEffect(() => {
    // Si la ruta NO está restringida, no hacemos nada.
    if (!rutaRestringida) {
      if (estado !== "idle") logInfo("Ruta no restringida: watcher inactivo.");
      setEstado("idle");
      return;
    }

    // Validación inicial: ¿hay token registrado?
    if (!existeTokenRegistrado()) {
      setEstado("no_registrado");
      logWarn("Ruta restringida pero no hay token registrado.");
      return;
    }

    logInfo("Watcher activo en ruta restringida.");

    const interval = window.setInterval(async () => {
      try {
        if (!tokenExpirado()) return;

        logWarn("Token expirado detectado.");

        if (authConfig.tokenStrategy === "redirect") {
          setEstado("expirado");

          if (authConfig.avisoDelayMs > 0) {
            timeoutRef.current = window.setTimeout(() => {
              logInfo("Redirigiendo por expiración.");
              finalizarSesionYRedirigir(navigate);
            }, authConfig.avisoDelayMs);
          } else {
            finalizarSesionYRedirigir(navigate);
          }
          return;
        }

        if (authConfig.tokenStrategy === "renew") {
          setEstado("renovando");
          try {
            logInfo("Intentando renovación de token...");
            await renovarToken();
            refrescarClaims();
            setEstado("idle");
            logInfo("Token renovado y claims refrescados.");
          } catch (err) {
            logError("Fallo renovación. Cerrando sesión.", err);
            finalizarSesionYRedirigir(navigate);
          }
        }
      } catch (err) {
        logError("Error inesperado en watcher.", err);
        finalizarSesionYRedirigir(navigate);
      }
    }, authConfig.checkIntervalMs);

    return () => window.clearInterval(interval);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rutaRestringida, renovarToken, refrescarClaims]);

  // 🔕 No renderizar nada mientras está validando o si el usuario no está registrado
  if (estado === "idle" || estado === "no_registrado") return null;

  return (
    <div
      className={styles.overlay}
      role="dialog"
      aria-modal="true"
      onClick={(e) => e.stopPropagation()}
      onWheel={(e) => e.preventDefault()}
      onTouchMove={(e) => e.preventDefault()}
    >
      <div className={styles.contenedor}>
        <div className={styles.mensaje}>
          <span className={styles.icono}>⚠️</span>

          {estado === "expirado" && (
            <span>Tu sesión ha caducado. Serás redirigido al iniciar sesión.</span>
          )}

          {estado === "renovando" && (
            <span>Renovando sesión, por favor espera…</span>
          )}
        </div>

        <button className={styles.botonLogin} onClick={() => navigate("/LoginPage")}>
          Iniciar sesión
        </button>
      </div>
    </div>
  );
}
