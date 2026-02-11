import { Box, IconButton, Typography } from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";

export interface SectionHeaderProps {
  title: string;
  open: boolean;
  onToggle: () => void;
}

export default function SectionHeader({
  title,
  open,
  onToggle,
}: SectionHeaderProps) {
  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        mb: 2,
        gap: 1,
      }}
    >
      <Typography
        variant="h6"
        sx={{ fontWeight: 600, flexGrow: 1 }}
      >
        {title}
      </Typography>

      <IconButton
        aria-label={open ? "Contraer sección" : "Expandir sección"}
        onClick={onToggle}
        size="small"
      >
        {open ? <ExpandLessIcon /> : <ExpandMoreIcon />}
      </IconButton>
    </Box>
  );
}
