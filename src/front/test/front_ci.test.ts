import { describe, test, expect } from 'vitest'
//import { setup } from '@nuxt/test-utils'

describe('My test', async () => {
  //const nuxt = await setup({})

  test('Vérification de 2+2', () => {
    // Ton test ici
    const result = 2 + 2
    expect(result).toBe(4)
  })

})