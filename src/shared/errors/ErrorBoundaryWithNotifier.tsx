import { ErrorBoundary } from "./ErrorBoundary";
import { useAppErrorNotifier } from "../hooks/useAppErrorNotifier";


export default function ErrorBoundaryWithNotifier({
  children,
  fallback,
}: {
  children: React.ReactNode;
  fallback?: React.ReactNode;
}) {
  const notifyAppError = useAppErrorNotifier();

  return (
    <ErrorBoundary
      fallback={fallback}
      onError={(error) => {
        notifyAppError(error, "Error inesperado en la aplicación");
      }}
    >
      {children}
    </ErrorBoundary>
  );
}
