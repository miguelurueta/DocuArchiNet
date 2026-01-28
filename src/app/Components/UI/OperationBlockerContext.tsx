import { createContext, useContext, useState } from "react";
import { Backdrop, CircularProgress, Typography, Stack } from "@mui/material";

type Ctx = {
  block: (message?: string) => void;
  unblock: () => void;
  isBlocked: boolean;
};

const OperationBlockerContext = createContext<Ctx | null>(null);

export function OperationBlockerProvider({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useState(false);
  const [message, setMessage] = useState("Procesando...");

  const block = (msg?: string) => {
    setMessage(msg ?? "Procesando...");
    setOpen(true);
  };

  const unblock = () => setOpen(false);

  return (
    <OperationBlockerContext.Provider value={{ block, unblock, isBlocked: open }}>
      {children}

      <Backdrop open={open} sx={{ zIndex: 2000, color: "#fff" }}>
        <Stack spacing={2} alignItems="center">
          <CircularProgress color="inherit" />
          <Typography>{message}</Typography>
        </Stack>
      </Backdrop>
    </OperationBlockerContext.Provider>
  );
}

export function useOperationBlocker() {
  const ctx = useContext(OperationBlockerContext);
  if (!ctx) throw new Error("OperationBlockerProvider missing");
  return ctx;
}
