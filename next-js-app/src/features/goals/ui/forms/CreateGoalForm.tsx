import { goalCreateSchemaType } from '@/entities/goal/model/schemas';
import { Checkbox } from '@/shared/components/ui/checkbox';
import { Collapsible, CollapsibleContent } from '@/shared/components/ui/collapsible';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/shared/components/ui/form';
import { Input } from '@/shared/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/shared/components/ui/select';
import { Textarea } from '@/shared/components/ui/textarea';
import { LifeArea } from '@/shared/types/api-types';
import { GenericFormProps } from '@/shared/types/form';

export const CreateGoalForm = ({ form, onSubmit, id }: GenericFormProps<goalCreateSchemaType>) => {
  const isSmartGoal = form.watch('isSmartGoal');

  return (
    <Form {...form}>
      <form id={id} onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 py-4">
        <FormField
          control={form.control}
          name="title"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Title</FormLabel>
              <FormControl>
                <Input placeholder="Eg. Learn React" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="lifeArea"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Life Area</FormLabel>
              <Select onValueChange={field.onChange} value={field.value || ''}>
                <FormControl>
                  <SelectTrigger className="w-full">
                    <SelectValue />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {Object.entries(LifeArea).map(([key, label]) => (
                    <SelectItem key={key} value={key}>
                      {label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="isActive"
          render={({ field }) => (
            <FormItem className="flex gap-3 items-center">
              <FormControl>
                <Checkbox checked={field.value} onCheckedChange={field.onChange} />
              </FormControl>
              <FormLabel>Set as Active Goal</FormLabel>
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="isSmartGoal"
          render={({ field }) => (
            <FormItem className="flex gap-3 items-center">
              <FormControl>
                <Checkbox checked={field.value} onCheckedChange={field.onChange} />
              </FormControl>
              <FormLabel>Make this a SMART Goal</FormLabel>
            </FormItem>
          )}
        />

        <Collapsible open={isSmartGoal}>
          <CollapsibleContent className="space-y-4 animate-collapsible-down">
            <h4 className="text-sm font-medium text-muted-foreground mb-4">SMART Criteria</h4>

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Specific</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="What exactly do you want to accomplish?"
                      className="resize-none"
                      {...field}
                      value={field.value || ''}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="metrics"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Measurable</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="How will you measure your progress?"
                      className="resize-none"
                      {...field}
                      value={field.value || ''}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="achievabilityRationale"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Achievable</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Why is this goal achievable?"
                      className="resize-none"
                      {...field}
                      value={field.value || ''}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="motivation"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Relevant</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Why is this goal important to you?"
                      className="resize-none"
                      {...field}
                      value={field.value || ''}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </CollapsibleContent>
        </Collapsible>
      </form>
    </Form>
  );
};
