import { Box, Paper, Typography } from "@mui/material";
import DashboardCards from "../components/DashboardCards";
import DashboardLayout from "../components/DashboardLayout";

/**
 * Página principal del dashboard con tarjetas y área de contenido.
 */
const DashboardPage = () => (
  <DashboardLayout>
    <DashboardCards />
    <Paper
      elevation={0}
      sx={{
        mt: 4,
        p: 4,
        minHeight: 320,
        borderRadius: 4,
        border: "1px dashed",
        borderColor: "divider",
        bgcolor: "#fff",
      }}
    >
      <Typography variant="h6" fontWeight={600} gutterBottom>
        Panel de trabajo
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Selecciona una opción del menú para visualizar su información y
        reportes asociados.
      </Typography>
    </Paper>
  </DashboardLayout>
);

export default DashboardPage;
