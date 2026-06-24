import { Box, Typography } from "@mui/material";
import type { ReactNode } from "react";

type PageHeaderProps = {
  title: string;
  action?: ReactNode;
  subtitle?: string;
};

const PageHeader = ({ title, action, subtitle }: PageHeaderProps) => (
  <Box
    sx={{
      display: "flex",
      flexDirection: { xs: "column", sm: "row" },
      alignItems: { xs: "stretch", sm: "center" },
      justifyContent: "space-between",
      gap: 2,
      mb: 2,
    }}
  >
    <Box>
      <Typography
        variant="h4"
        sx={{ fontWeight: 700, fontSize: { xs: "1.5rem", sm: "2.125rem" }, lineHeight: 1.2 }}
      >
        {title}
      </Typography>
      {subtitle && (
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          {subtitle}
        </Typography>
      )}
    </Box>
    {action}
  </Box>
);

export default PageHeader;
