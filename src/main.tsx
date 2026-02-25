
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./app/styles/RequiredTooltip.css";
import "./shared/Style/global.css"
import "@fortawesome/fontawesome-free/css/all.min.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { OperationBlockerProvider } from "./app/Components/UI/OperationBlockerContext.tsx";

const queryClient = new QueryClient({
  defaultOptions: {
    mutations: { retry: false },
    queries: { refetchOnWindowFocus: false, retry: false },
  },
});

document.documentElement.setAttribute("translate", "no");
document.body?.classList.add("notranslate");

createRoot(document.getElementById("root")!).render(
  <QueryClientProvider client={queryClient}>
    <OperationBlockerProvider>
      <App />
    </OperationBlockerProvider>
  </QueryClientProvider>
);
