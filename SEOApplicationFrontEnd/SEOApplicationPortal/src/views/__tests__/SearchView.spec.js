import { mount } from '@vue/test-utils'
import axios from 'axios'
import SearchView from '@/views/SearchView.vue'
import { describe, it, expect, beforeEach, vi } from 'vitest'

vi.mock('axios')

describe('SearchView.vue', () => {
  let wrapper

  beforeEach(() => {
    wrapper = mount(SearchView)
  })

  it('should render the view correctly', () => {
    expect(wrapper.exists()).toBe(true)
  })

  it('should call the API and update the result when onSearch is called', async () => {
    axios.get.mockResolvedValueOnce({ data: 'Search result data' })

    await wrapper.setData({
      searchPhrase: 'test search phrase',
      targetUrl: 'https://test.com',
    })

    await wrapper.find('form').trigger('submit.prevent')

    await wrapper.vm.$nextTick()

    expect(wrapper.vm.result).toBe('Search result data')
  })

  it('should display loading message when search is in progress', async () => {
    wrapper.setData({ searchPhrase: 'test', targetUrl: 'https://test.com' })
    const searchPromise = wrapper.vm.onSearch()

    expect(wrapper.vm.loading).toBe(true)
  })

  it('should display error message when there is an error during the search', async () => {
    axios.get.mockRejectedValue(new Error('Network Error'))

    wrapper.setData({ searchPhrase: 'test', targetUrl: 'https://test.com' })
    await wrapper.vm.onSearch()

    expect(wrapper.vm.error).toBe(true)

    expect(wrapper.find('.error-message').exists()).toBe(true)
  })

  it('should display "No results found" if result is "0"', async () => {
    axios.get.mockResolvedValue({ data: '0' })

    wrapper.setData({ searchPhrase: 'test', targetUrl: 'https://test.com' })
    await wrapper.vm.onSearch()

    expect(wrapper.vm.result).toBe('0')

    expect(wrapper.find('.empty-result').exists()).toBe(true)
  })

  it('should display search result when response is valid', async () => {
    const mockData = 'Search result data'
    axios.get.mockResolvedValue({ data: mockData })

    wrapper.setData({ searchPhrase: 'test', targetUrl: 'https://test.com' })
    await wrapper.vm.onSearch()

    expect(wrapper.vm.result).toBe(mockData)

    expect(wrapper.find('.result-container').exists()).toBe(true)
    expect(wrapper.find('.result-text').text()).toBe(mockData)
  })

  it('should not display result if there is no result', async () => {
    axios.get.mockResolvedValue({ data: null })

    wrapper.setData({ searchPhrase: 'test', targetUrl: 'https://test.com' })
    await wrapper.vm.onSearch()

    expect(wrapper.vm.result).toBeNull()

    expect(wrapper.find('.result-container').exists()).toBe(false)
  })

  it('should fetch rankings when component is mounted', async () => {
    axios.get.mockResolvedValueOnce({
      data: {
        ranks: [
          {
            searchPhrase: 'test',
            url: 'https://test1.com',
            rank: 1,
            searchedOn: '2024-01-01T10:00:00Z',
          },
          {
            searchPhrase: 'test',
            url: 'https://test2.com',
            rank: 2,
            searchedOn: '2024-01-02T10:00:00Z',
          },
        ],
      },
    })

    wrapper = mount(SearchView)

    await wrapper.vm.$nextTick()

    expect(wrapper.vm.ranks).toHaveLength(2)
    expect(wrapper.vm.ranks[0].searchedOn).toBe('2024-01-02T10:00:00Z')
  })

  it('should render rankings table with correct data', async () => {
    axios.get.mockResolvedValueOnce({
      data: {
        ranks: [
          {
            searchPhrase: 'test',
            url: 'https://test1.com',
            rank: 1,
            searchedOn: '2024-01-01T10:00:00Z',
          },
          {
            searchPhrase: 'test',
            url: 'https://test2.com',
            rank: 2,
            searchedOn: '2024-01-02T10:00:00Z',
          },
        ],
      },
    })

    wrapper = mount(SearchView)
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()

    const rows = wrapper.findAll('tbody tr')

    expect(rows.length).toBe(2)
    expect(rows.at(0).text()).toContain('https://test2.com')
    expect(rows.at(1).text()).toContain('https://test1.com')
  })

  it('should format the date correctly', () => {
    const date = '2024-01-01T10:00:00Z'
    const formattedDate = wrapper.vm.formatDate(date)
    const expectedDate = new Date(date).toLocaleString()
    expect(formattedDate).toBe(expectedDate)
  })
})
