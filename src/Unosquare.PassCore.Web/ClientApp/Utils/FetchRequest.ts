export async function fetchRequest(url: string, requestMethod: string, requestBody?: any) {
    const headers: Headers = new Headers();
    headers.append('Accept', 'application/json');
    headers.append('Content-Type', 'application/json');

    const init = {
        body: requestBody ? requestBody : null,
        headers,
        method: requestMethod,
    };

    const request = new Request(url, init);
    const response = await fetch(request);
    const responseBody = await response.text();

    return responseBody ? JSON.parse(responseBody) : {};
}
