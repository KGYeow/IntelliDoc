<template>
  <form @submit.prevent="deleteUser">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col class="pb-0">
            <v-label class="text-caption">Your Account Password</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="passwordConfirmationInfo.password.value"
              :error-messages="passwordConfirmationInfo.password.errorMessage"
              :append-inner-icon="passwordVisible ? 'mdi-eye-off fs-5' : 'mdi-eye fs-5'"
              :type="passwordVisible ? 'text' : 'password'"
              hide-details="auto"
              @click:append-inner="passwordVisible = !passwordVisible"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-label class="text-caption">Confirm Password</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="passwordConfirmationInfo.confirmPassword.value"
              :error-messages="passwordConfirmationInfo.confirmPassword.errorMessage"
              :append-inner-icon="confirmPasswordVisible ? 'mdi-eye-off fs-5' : 'mdi-eye fs-5'"
              :type="confirmPasswordVisible ? 'text' : 'password'"
              hide-details="auto"
              @click:append-inner="confirmPasswordVisible = !confirmPasswordVisible"
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">  
      <v-btn color="primary" type="submit">Submit</v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'

// Properties, Emit & Model
const props = defineProps({
  userId: Number,
})
const emit = defineEmits(['close-modal'])

// Data
const { handleSubmit } = useForm({
  validationSchema: {
    password(value) {
      return value ? true : 'Your account password is required'
    },
    confirmPassword(value) {
      if (value) {
        if (value != passwordConfirmationInfo.value.password.value)
          return 'The confirmed password does not match your account password'
        return true
      }
      else
        return 'Confirm password is required'
    },
  }
})
const passwordConfirmationInfo = ref({
  password: useField('password'),
  confirmPassword: useField('confirmPassword'),
})
const passwordVisible = ref(false)
const confirmPasswordVisible = ref(false)

// Methods
const deleteUser = handleSubmit(async(values) => {
  try {
    const result = await useFetchCustom.$delete(`/User/${props.userId}/${values.password}`)
    if (!result.error) {
      emit('close-modal', false)
      passwordConfirmationInfo.value.password.resetField()
      passwordConfirmationInfo.value.confirmPassword.resetField()
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
})
</script>