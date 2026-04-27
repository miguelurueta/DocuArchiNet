import { LeftOutlined, LoadingOutlined, ToolOutlined } from "@ant-design/icons";
import { cloneElement, isValidElement } from "react";
import type { ReactElement, ReactNode } from "react";
import { useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppLoadingState } from "../../../app/Components/UI/AppLoadingState";
import { useEstructuraRespuestaIdTarea } from "../hooks/useEstructuraRespuestaIdTarea";
import GestionCorrespondenciaRoutePage from "../pages/GestionCorrespondenciaRoutePage";
import styles from "../style/GestionCorrespondenciaRoute.module.css";

interface GestionCorrespondenciaRouteProps {
  detailContent?: ReactNode;
}

type DetailState = "loading" | "ready" | "blocked-empty";
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
    isEmptyLatched,
    resolved,
  } = useEstructuraRespuestaIdTarea(hasDetail && hasValidId ? parsedId : undefined);

  // Invalid ids should not keep a sticky detail panel; errors should show a blocked state.
  const shouldAutoClose = hasDetail && !hasValidId;

  useEffect(() => {
    if (!shouldAutoClose) return;
    navigate("/dashboard/gestion-correspondencia", { replace: true });
  }, [navigate, shouldAutoClose]);

  const detailState: DetailState = !hasDetail
    ? "ready"
    : loading || !resolved
      ? "loading"
      : (isEmpty || isEmptyLatched)
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

  const blockedMessage = hasValidId
    ? Boolean(error)
      ? `No fue posible cargar la estructura para la tarea (IdTareaWf: ${parsedId}). ${String(
          error?.message ?? "",
        )}`.trim()
      : `No existe estructura disponible para la tarea (IdTareaWf: ${parsedId}).`
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

          <div
            className={styles.detailBody}
            data-detail-state={detailState}
            data-estructura-loading={loading ? "true" : "false"}
            data-estructura-resolved={resolved ? "true" : "false"}
            data-estructura-empty={isEmpty ? "true" : "false"}
            data-estructura-error={error ? "true" : "false"}
          >
            {isReady ? (
              detailContentWithContext
            ) : detailState === "loading" ? (
              <div className={styles.loadingState} data-testid="gestion-correspondencia-loading-state">
                <AppLoadingState
                  loading={detailState === "loading"}
                  delayMs={500}
                  title="Cargando estructura de la tarea"
                  message="Validando información…"
                  icon={
                    <>
                      <LoadingOutlined spin />
                      <ToolOutlined style={{ marginLeft: 8 }} />
                    </>
                  }
                />
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
