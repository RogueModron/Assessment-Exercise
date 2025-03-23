import { Dialog, TextField } from "@mui/material";

export interface TextDialogProps {
  onClose: () => void;
  open: boolean;
  text: string;
}

export default function TextDialog(props: TextDialogProps) {
  const { onClose, open, text } = props;

  const handleClose = () => {
    onClose();
  };

  return (
    <Dialog fullWidth onClose={handleClose} open={open}>
      <TextField multiline maxRows={10} value={text} sx={{ m: 2 }} />
    </Dialog>
  );
}
