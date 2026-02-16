import { Skeleton, Stack, Alert } from "@mui/material";
import { RadicacionPage } from "./RadicacionPage";
import { EMPTY_PLANTILLA_RADICADO } from "../services/radicacionDefaults";
import { useCamposPlantilla } from "../hooks/useCamposPlantilla";
import { mapCamposPlantillaToPlantillaRadicado } from "../services/mapCamposPlantillaToPlantillaRadicado";

export default function RadicacionRoutePage() {
  const { data, isLoading, error } = useCamposPlantilla();

  if (isLoading) {
    return (
      <Stack spacing={1.5}>
        <Skeleton variant="rounded" height={40} />
        <Skeleton variant="rounded" height={40} />
        <Skeleton variant="rounded" height={120} />
        <Skeleton variant="rounded" height={40} />
      </Stack>
    );
  }

  if (error) {
    return <Alert severity="error">No fue posible cargar la plantilla de radicación.</Alert>;
  }

  const plantilla =
    data.length > 0
      ? mapCamposPlantillaToPlantillaRadicado(data)
      : EMPTY_PLANTILLA_RADICADO;

  return <RadicacionPage plantilla={plantilla} />;
}
