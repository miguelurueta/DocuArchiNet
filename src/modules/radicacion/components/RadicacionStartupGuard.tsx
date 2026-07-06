import { useEffect, useState, type ReactNode } from "react";
import { Alert, Button, Skeleton, Stack } from "@mui/material";
import { useRadicacionDocumentalContext } from "../hooks/useRadicacionDocumentalContext";
import { useRadicacionEstadoActivo } from "../hooks/useRadicacionEstadoActivo";

interface RadicacionStartupGuardProps {
  children: ReactNode;
}

export function RadicacionStartupGuard({
  children,
}: RadicacionStartupGuardProps) {
  const { setContextoDocumental, clearContextoDocumental } =
    useRadicacionDocumentalContext();
  const {
    contextoDocumental,
    isLoading,
    isFetching,
    isError,
    refetch,
  } = useRadicacionEstadoActivo();
  const [isBootstrapped, setIsBootstrapped] = useState(false);

  useEffect(() => {
    if (isLoading || isFetching || isError || isBootstrapped) {
      return;
    }

    if (contextoDocumental) {
      setContextoDocumental(contextoDocumental);
    } else {
      clearContextoDocumental();
    }

    // El render funcional se desbloquea solo despues de restaurar/limpiar el contexto.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setIsBootstrapped(true);
  }, [
    clearContextoDocumental,
    contextoDocumental,
    isBootstrapped,
    isError,
    isFetching,
    isLoading,
    setContextoDocumental,
  ]);

  if (isError) {
    return (
      <Alert
        severity="error"
        action={
          <Button
            color="inherit"
            size="small"
            onClick={() => {
              setIsBootstrapped(false);
              void refetch();
            }}
          >
            Reintentar
          </Button>
        }
      >
        No fue posible verificar el estado activo de radicación.
      </Alert>
    );
  }

  if (isLoading || isFetching || !isBootstrapped) {
    return (
      <Stack spacing={1.5}>
        <Skeleton variant="rounded" height={40} />
        <Skeleton variant="rounded" height={40} />
        <Skeleton variant="rounded" height={120} />
      </Stack>
    );
  }

  return <>{children}</>;
}
