import { HTMLAttributes } from 'react';
import { FieldValues, UseFormReturn } from 'react-hook-form';

export interface GenericFormProps<TFormValues extends FieldValues> extends Omit<HTMLAttributes<HTMLFormElement>, 'onSubmit'> {
  form: UseFormReturn<TFormValues>;
  onSubmit: (values: TFormValues) => void;
}
