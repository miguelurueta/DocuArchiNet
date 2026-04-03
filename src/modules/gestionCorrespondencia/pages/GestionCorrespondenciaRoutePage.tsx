import { Alert } from "@mui/material";
import GestionCorrespondencia from "./GestionCorrespondencia";
import GestionCorrespondenciaTableSkeleton from "../components/GestionCorrespondenciaTableSkeleton";
import { useGestionCorrespondenciaTable } from "../hooks/useGestionCorrespondenciaTable";

export default function GestionCorrespondenciaRoutePage() {
  const table = useGestionCorrespondenciaTable();

  if (!table.hasLoadedOnce && table.loading) {
    return <GestionCorrespondenciaTableSkeleton />;
  }

  if (table.error) {
    return (
      <Alert severity="error">
        No fue posible cargar la bandeja de gestión de correspondencia.
      </Alert>
    );
  }

  return <GestionCorrespondencia table={table} />;
}
