import {

  Box,

  Button,

  Container,

  Grid,

  Stack,

  Typography,

} from "@mui/material";

import { Link as RouterLink } from "react-router-dom";

import GavelIcon from "@mui/icons-material/Gavel";



const pagePadding = { px: { xs: 2, sm: 3, md: 4 } };



const features = [

  {

    title: "Firm-branded landing pages",

    body: "Each firm gets a public site at their code, e.g. /DEMO, with carousel photos, about, practice areas, and advocate profiles.",

  },

  {

    title: "Case & court workflow",

    body: "Structured case records, hearing diary, court-wise organization, documents, and payment tracking.",

  },

  {

    title: "Multi-tenant SaaS",

    body: "Isolated data per firm with administrator-managed website content and staff access controls.",

  },

];



const LandingPage = () => (

  <>

    <Box

      sx={{

        minHeight: { xs: 320, sm: 420 },

        background: "linear-gradient(135deg, #0d1b2a 0%, #1b263b 50%, #415a77 100%)",

        color: "#fff",

        py: { xs: 5, sm: 8 },

      }}

    >

      <Container maxWidth={false} sx={pagePadding}>

        <Typography variant="overline" sx={{ color: "#c9a227", letterSpacing: 2 }}>

          Legal Practice Management SaaS

        </Typography>

        <Typography

          variant="h2"

          sx={{ mt: 1, fontWeight: 700, fontSize: { xs: "1.75rem", sm: "2.5rem", md: "3.75rem" }, lineHeight: 1.15 }}

        >

          Modern case management for law firms

        </Typography>

        <Typography variant="h6" sx={{ mt: 2, maxWidth: 720, opacity: 0.9, fontSize: { xs: "1rem", sm: "1.25rem" } }}>

          Courts, cases, hearings, documents, and accounts — multi-tenant, secure, and ready for the web.

        </Typography>

        <Stack direction={{ xs: "column", sm: "row" }} spacing={2} sx={{ mt: 4 }}>

          <Button

            component={RouterLink}

            to="/DEMO"

            variant="contained"

            size="large"

            sx={{ width: { xs: "100%", sm: "auto" }, bgcolor: "#c9a227", color: "#0d1b2a", "&:hover": { bgcolor: "#b8921f" } }}

          >

            View Demo Firm Site

          </Button>

          <Button

            component={RouterLink}

            to="/login"

            variant="outlined"

            size="large"

            sx={{ width: { xs: "100%", sm: "auto" }, borderColor: "#c9a227", color: "#c9a227" }}

          >

            Staff Login

          </Button>

        </Stack>

      </Container>

    </Box>



    <Box sx={{ py: { xs: 5, sm: 8 } }}>

      <Container maxWidth={false} sx={pagePadding}>

        <Grid container spacing={4} sx={{ width: "100%" }}>

          {features.map((item) => (

            <Grid key={item.title} size={{ xs: 12, md: 4 }}>

              <GavelIcon sx={{ fontSize: 40, color: "#c9a227" }} />

              <Typography variant="h6" sx={{ mt: 2, fontWeight: 700 }}>

                {item.title}

              </Typography>

              <Typography color="text.secondary">{item.body}</Typography>

            </Grid>

          ))}

        </Grid>

      </Container>

    </Box>

  </>

);



export default LandingPage;


