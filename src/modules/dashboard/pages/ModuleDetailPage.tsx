import { Box, Chip, Paper, Typography } from "@mui/material";
import { useParams } from "react-router";
import DashboardLayout from "../components/DashboardLayout";
import { useMenuItems } from "../hooks/useMenuItems";

/**
 * Página de detalle para cada módulo seleccionado en el sidebar.
 */
const ModuleDetailPage = () => {
  const { nodeId } = useParams();
  const { data } = useMenuItems();
  const current = data?.find((item) => item.ValueNode === nodeId);

  return (
    <DashboardLayout>
      <Paper
        elevation={0}
        sx={{
          p: { xs: 3, md: 4 },
          borderRadius: 4,
          border: "1px solid",
          borderColor: "divider",
          bgcolor: "#fff",
        }}
      >
        <Typography variant="h5" fontWeight={700} gutterBottom>
          {current?.NombreModulo ?? "Módulo"}
        </Typography>
        <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1, mb: 2 }}>
          {current?.ValueNode && <Chip label={current.ValueNode} />}
          {current?.TIpoModulo && <Chip label={current.TIpoModulo} />}
        </Box>
        <Typography variant="body2" color="text.secondary">
          {current?.ToltipNode ||
            "Este módulo está listo para integrarse con sus datos y reportes."}
        </Typography>
      </Paper>
    </DashboardLayout>
  );
};

export default ModuleDetailPage;
