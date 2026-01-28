import React from "react";

type Props = {
  children: React.ReactNode;
  fallback?: React.ReactNode;
  onError?: (error: Error) => void;
};

type State = {
  hasError: boolean;
};

export class ErrorBoundary extends React.Component<Props, State> {
  state: State = { hasError: false };

  // 1️⃣ Activa el fallback
  static getDerivedStateFromError(): State {
    return { hasError: true };
  }

  // 2️⃣ Permite ejecutar lógica (CRÍTICO)
  componentDidCatch(error: Error, info: React.ErrorInfo) {
    console.error("🔥 ErrorBoundary capturó un error:", error);
    console.error("ℹ️ Info:", info);

    // 🔌 Delegación al wrapper
    this.props.onError?.(error);
  }

  render() {
    if (this.state.hasError) {
      return (
        this.props.fallback ?? (
          <div style={{ padding: 24 }}>
            <h2>⚠️ Error inesperado</h2>
            <p>Ocurrió un problema en la interfaz.</p>
          </div>
        )
      );
    }

    return this.props.children;
  }
}
