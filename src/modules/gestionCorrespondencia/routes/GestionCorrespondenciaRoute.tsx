import { LeftOutlined } from "@ant-design/icons";
import { cloneElement, isValidElement } from "react";
import type { ReactElement, ReactNode } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { useEstructuraRespuestaIdTarea } from "../hooks/useEstructuraRespuestaIdTarea";
import GestionCorrespondenciaRoutePage from "../pages/GestionCorrespondenciaRoutePage";
import styles from "../style/GestionCorrespondenciaRoute.module.css";

interface GestionCorrespondenciaRouteProps {
  detailContent?: ReactNode;
}

type DetailState = "loading" | "ready" | "blocked-empty" | "blocked-error" | "blocked-invalid-id";
type DetailContentContextProps = {
  idTareaWf?: number;
  detailState?: DetailState;
};

export default function GestionCorrespondenciaRoute({
  detailContent,
}: GestionCorrespondenciaRouteProps) {
  const navigate = useNavigate();
  const params = useParams();
  const parsedId = Number.parseInt(params.id ?? "", 10);
  const hasDetail = Boolean(detailContent);
  const hasValidId = Number.isFinite(parsedId) && parsedId > 0;

  const {
    estrucTuraRespuesta,
    loading,
    error,
    isEmpty,
  } = useEstructuraRespuestaIdTarea(hasDetail && hasValidId ? parsedId : undefined);

  const detailState: DetailState = !hasDetail
    ? "ready"
    : !hasValidId
      ? "blocked-invalid-id"
      : loading
        ? "loading"
        : error
          ? "blocked-error"
          : isEmpty
            ? "blocked-empty"
            : "ready";

  const isReady = detailState === "ready";
  const detailContentWithContext =
    hasDetail && isReady && isValidElement(detailContent)
      ? cloneElement(
          detailContent as ReactElement<DetailContentContextProps>,
          {
            idTareaWf: parsedId,
            detailState,
          },
        )
      : null;

  const blockedMessage =
    detailState === "blocked-invalid-id"
      ? "No se pudo resolver una tarea valida para cargar la gestion de respuesta."
      : detailState === "blocked-error"
        ? "No fue posible consultar la estructura de la tarea. Vuelve a la bandeja para reintentar."
        : "No existe estructura disponible para esta tarea de gestion respuesta.";

  const metadata = [
    { label: "Radicado", value: loading ? "..." : (isReady ? (estrucTuraRespuesta?.Radicado ?? "-") : "-") },
    {
      label: "Remitente",
      value: loading ? "..." : (isReady ? (estrucTuraRespuesta?.Destinatario ?? "-") : "-"),
    },
    { label: "Tramite", value: isReady ? (estrucTuraRespuesta?.TramiteDocumento ?? "-") : "-" },
  ];

  const handleClose = () => {
    navigate("/dashboard/gestion-correspondencia");
  };

  return (
    <section
      className={`${styles.shell} ${hasDetail ? styles.shellWithDetail : ""}`.trim()}
      data-testid="gestion-correspondencia-route-shell"
    >
      <div className={styles.mainRegion} data-testid="gestion-correspondencia-main-region">
        <GestionCorrespondenciaRoutePage />
      </div>

      {hasDetail ? (
        <aside
          className={styles.detailRegion}
          aria-label="Panel superpuesto de gestion de correspondencia"
          data-testid="gestion-correspondencia-detail-region"
        >
          <header className={styles.detailHeader}>
            <div className={styles.detailHeaderStart}>
              <AppButton
                aria-label="Volver a la bandeja"
                className={styles.detailReturnAction}
                icon={<LeftOutlined />}
                size="sm"
                tooltip="Volver a la bandeja"
                variant="ghost"
                onClick={handleClose}
              />

              <div className={styles.detailMeta} aria-label="Metadata de la respuesta">
                {metadata.map((item) => (
                  <span className={styles.detailMetaItem} key={item.label}>
                    <strong>{item.label}:</strong>
                    <span title={item.value}>{item.value}</span>
                  </span>
                ))}
              </div>
            </div>
          </header>

          <div className={styles.detailBody}>
            {isReady ? (
              detailContentWithContext
            ) : detailState === "loading" ? (
              <div className={styles.blockedState} data-testid="gestion-correspondencia-loading-state">
                <h3 className={styles.blockedTitle}>Cargando estructura de la tarea</h3>
                <p className={styles.blockedCopy}>
                  Espera un momento mientras validamos la informacion de la gestion respuesta.
                </p>
              </div>
            ) : (
              <div className={styles.blockedState} data-testid="gestion-correspondencia-blocked-state">
                <h3 className={styles.blockedTitle}>Gestion respuesta bloqueada</h3>
                <p className={styles.blockedCopy}>{blockedMessage}</p>
                <AppButton size="sm" variant="secondary" onClick={handleClose}>
                  Volver a bandeja
                </AppButton>
              </div>
            )}
          </div>
        </aside>
      ) : null}
    </section>
  );
}
